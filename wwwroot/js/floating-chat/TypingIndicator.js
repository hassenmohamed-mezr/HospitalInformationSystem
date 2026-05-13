/**
 * Typing / "thinking" indicator (animated dots).
 */
export function createTypingIndicator(hostEl) {
  const root = document.createElement("div");
  root.className = "floating-chat__typing";
  root.setAttribute("role", "status");
  root.setAttribute("aria-live", "polite");
  root.setAttribute("aria-atomic", "true");
  root.innerHTML = `
    <span class="floating-chat__typing-label">Thinking</span>
    <span class="floating-chat__typing-dot" aria-hidden="true"></span>
    <span class="floating-chat__typing-dot" aria-hidden="true"></span>
    <span class="floating-chat__typing-dot" aria-hidden="true"></span>
  `;
  hostEl.appendChild(root);

  return {
    show() {
      root.classList.add("is-visible");
    },
    hide() {
      root.classList.remove("is-visible");
    },
    destroy() {
      root.remove();
    },
  };
}
