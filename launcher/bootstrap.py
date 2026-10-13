import os
import subprocess
import time
from pathlib import Path

import requests


class CoreProcess:
    def __init__(self, config: dict, project_root: Path):
        self.cfg = config["core"]
        self.root = project_root
        self.proc = None
        self.base_url = f"http://{self.cfg['host']}:{self.cfg['port']}"

    def start(self, on_log=print):
        project_dir = self.root / self.cfg["project_dir"]
        if not project_dir.exists():
            raise RuntimeError(f"C# проект не найден: {project_dir}")

        on_log(f"[core] запускаю dotnet run в {project_dir}")
        env = os.environ.copy()
        env["ASPNETCORE_URLS"] = self.base_url
        env["ASPNETCORE_ENVIRONMENT"] = "Development"

        creation_flags = subprocess.CREATE_NEW_PROCESS_GROUP if os.name == "nt" else 0

        self.proc = subprocess.Popen(
            ["dotnet", "run", "--no-build", "-c", "Release"],
            cwd=str(project_dir),
            env=env,
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            creationflags=creation_flags,
            bufsize=1,
            universal_newlines=True,
        )

        deadline = time.time() + self.cfg["startup_timeout_sec"]
        while time.time() < deadline:
            if self.proc.poll() is not None:
                raise RuntimeError(
                    f"C# процесс упал с кодом {self.proc.returncode}"
                )
            try:
                r = requests.get(f"{self.base_url}/health", timeout=1)
                if r.status_code == 200:
                    on_log(f"[core] online на {self.base_url}")
                    return
            except requests.RequestException:
                pass
            time.sleep(self.cfg["health_poll_sec"])

        raise RuntimeError("C# не ответил за отведённое время")

    def is_alive(self):
        if self.proc is None or self.proc.poll() is not None:
            return False
        try:
            return requests.get(
                f"{self.base_url}/health", timeout=1
            ).status_code == 200
        except requests.RequestException:
            return False

    def stop(self):
        if self.proc is None:
            return
        try:
            if self.proc.poll() is None:
                self.proc.terminate()
                try:
                    self.proc.wait(timeout=3)
                except subprocess.TimeoutExpired:
                    self.proc.kill()
        except Exception:
            pass
