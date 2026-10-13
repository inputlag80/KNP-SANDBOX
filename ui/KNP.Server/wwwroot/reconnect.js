(function () {
    const css = `
        .components-reconnect-dialog { display: none !important; }
        .knp-reconnect {
            position: fixed;
            top: 0; left: 0; right: 0; bottom: 0;
            background: rgba(8, 5, 15, 0.9);
            backdrop-filter: blur(6px);
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            gap: 24px;
            z-index: 99999;
            font-family: system-ui, sans-serif;
        }
        .knp-reconnect-spinner {
            width: 64px;
            height: 64px;
            border-radius: 50%;
            border: 3px solid rgba(168, 85, 247, 0.2);
            border-top-color: #a855f7;
            animation: knp-spin 0.9s linear infinite;
        }
        .knp-reconnect-text {
            color: #c4c4cc;
            font-size: 14px;
            letter-spacing: 0.5px;
            white-space: nowrap;
        }
        @keyframes knp-spin {
            from { transform: rotate(0deg); }
            to   { transform: rotate(360deg); }
        }
    `;

    function apply() {
        const m = document.getElementById('components-reconnect-modal');
        if (!m || !m.shadowRoot) return;
        const sr = m.shadowRoot;

        if (!sr.querySelector('style[data-knp]')) {
            const s = document.createElement('style');
            s.setAttribute('data-knp', '1');
            s.textContent = css;
            sr.appendChild(s);
        }

        if (!sr.querySelector('.knp-reconnect')) {
            const box = document.createElement('div');
            box.className = 'knp-reconnect';
            box.innerHTML = '<div class="knp-reconnect-spinner"></div>'
                          + '<div class="knp-reconnect-text">Подключение к серверу…</div>';
            sr.appendChild(box);
        }
    }

    new MutationObserver(apply).observe(document.body, { childList: true, subtree: true });
    setInterval(apply, 300);
})();