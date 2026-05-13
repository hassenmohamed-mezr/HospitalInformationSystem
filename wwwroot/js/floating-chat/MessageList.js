/**
 * Scrollable message list with safe text rendering.
 */
function escapeHtml(text) {
  const d = document.createElement("div");
  d.textContent = text;
  return d.innerHTML;
}

function formatAssistantText(text) {
  const escaped = escapeHtml(text);
  return escaped.replace(/\n/g, "<br>");
}

function welcomeHtml() {
  return `<p id="floating-chat-empty-hint" class="floating-chat__empty-hint small">Ask about appointments, visits, roles, and how to move through the system.</p>`;
}

export function createMessageList(containerEl) {
  const scroll = () => {
    containerEl.scrollTop = containerEl.scrollHeight;
  };

  return {
    appendUser(text) {
      const wrap = document.createElement("div");
      wrap.className = "floating-chat__msg floating-chat__msg--user";
      wrap.innerHTML = `
        <div class="floating-chat__msg-meta">You</div>
        <div class="floating-chat__msg-body">${escapeHtml(text)}</div>
      `;
      containerEl.appendChild(wrap);
      scroll();
    },

    appendAssistant(text) {
      const wrap = document.createElement("div");
      wrap.className = "floating-chat__msg floating-chat__msg--assistant";
      wrap.innerHTML = `
        <div class="floating-chat__msg-meta">Assistant</div>
        <div class="floating-chat__msg-body">${formatAssistantText(text)}</div>
      `;
      containerEl.appendChild(wrap);
      scroll();
    },

    appendError(text) {
      const wrap = document.createElement("div");
      wrap.className = "floating-chat__msg floating-chat__msg--error";
      wrap.innerHTML = `
        <div class="floating-chat__msg-meta">Could not reply</div>
        <div class="floating-chat__msg-body">${escapeHtml(text)}</div>
      `;
      containerEl.appendChild(wrap);
      scroll();
    },

    clear() {
      containerEl.innerHTML = welcomeHtml();
    },

    scrollToEnd: scroll,
  };
}
