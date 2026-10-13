import atexit
import json
import sys
import time
from pathlib import Path

import requests
from flask import Flask, jsonify, render_template_string, request

sys.path.insert(0, str(Path(__file__).parent))
from bootstrap import CoreProcess

ROOT = Path(__file__).resolve().parent.parent
CONFIG = json.loads(
    (Path(__file__).parent / "config.json").read_text(encoding="utf-8")
)

app = Flask(__name__)
core = CoreProcess(CONFIG, ROOT)


SPLASH = """<!doctype html>
<html><head><meta charset="utf-8"><title>KNP Sandbox</title>
<style>
  html,body{margin:0;height:100%;background:#08050f;color:#f5f3ff;
            font-family:system-ui,sans-serif;display:flex;
            align-items:center;justify-content:center}
  .box{text-align:center}
  h1{font-weight:600;letter-spacing:0.5px;color:#a855f7}
  .bar{margin-top:24px;width:280px;height:4px;background:#1e1530;
       border-radius:2px;overflow:hidden;margin-left:auto;margin-right:auto}
  .bar>div{height:100%;width:30%;background:#a855f7;border-radius:2px;
           animation:slide 1.4s ease-in-out infinite}
  @keyframes slide{0%{margin-left:-30%}100%{margin-left:100%}}
  .hint{margin-top:16px;color:#8a8a93;font-size:12px}
</style></head>
<body><div class="box">
  <h1>KNP Sandbox</h1>
  <div class="bar"><div></div></div>
  <div class="hint">Запуск ядра…</div>
</div>
<script>
  const poll = setInterval(async () => {
    try {
      const r = await fetch('/ready');
      const j = await r.json();
      if (j.ready) { clearInterval(poll); window.location = j.redirect; }
    } catch(e) {}
  }, 500);
</script>
</body></html>"""


@app.route("/")
def splash():
    return render_template_string(SPLASH)


@app.route("/ready")
def ready():
    if core.is_alive():
        url = f"http://{CONFIG['core']['host']}:{CONFIG['core']['port']}"
        return jsonify(ready=True, redirect=url)
    return jsonify(ready=False)


@app.route("/api/<path:subpath>", methods=["GET", "POST", "DELETE"])
def proxy(subpath):
    url = f"http://{CONFIG['core']['host']}:{CONFIG['core']['port']}/api/{subpath}"
    try:
        if request.method == "GET":
            r = requests.get(url, params=request.args, timeout=10)
        elif request.method == "POST":
            r = requests.post(url, json=request.get_json(silent=True), timeout=10)
        else:
            r = requests.delete(url, timeout=10)
        return (
            r.content,
            r.status_code,
            {"Content-Type": r.headers.get("Content-Type", "application/json")},
        )
    except requests.RequestException as e:
        return jsonify(error=str(e)), 502


def _shutdown():
    print("[flask] останавливаю C#...")
    core.stop()


atexit.register(_shutdown)


if __name__ == "__main__":
    print("[flask] старт bootstrap...")
    try:
        core.start(on_log=lambda m: print(m, flush=True))
    except Exception as e:
        print(f"[flask] ОШИБКА запуска C#: {e}")
        sys.exit(1)

    print(
        f"[flask] splash на "
        f"http://{CONFIG['flask']['host']}:{CONFIG['flask']['port']}"
    )
    app.run(
        host=CONFIG["flask"]["host"],
        port=CONFIG["flask"]["port"],
        debug=False,
        threaded=True,
        use_reloader=False,
    )