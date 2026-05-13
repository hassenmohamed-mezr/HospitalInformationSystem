import { createTypingIndicator } from "./TypingIndicator.js";
import { createMessageList } from "./MessageList.js";
import { createChatWindow } from "./ChatWindow.js";
import { wireFloatingChatButton } from "./FloatingChatButton.js";

/**
 * Wires the floating assistant: messages, API, loading state, minimize vs close.
 */
export function initFloatingChat(root) {
  const askUrl = root.dataset.chatAskUrl;
  if (!askUrl) return;

  const fab = root.querySelector("#floating-chat-fab");
  const panel = root.querySelector("#floating-chat-panel");
  const messagesEl = root.querySelector("#floating-chat-messages");
  const typingHost = root.querySelector("#floating-chat-typing-host");
  const input = root.querySelector("#floating-chat-input");
  const sendBtn = root.querySelector("#floating-chat-send");
  const btnMinimize = root.querySelector("#floating-chat-minimize");
  const btnClose = root.querySelector("#floating-chat-close");

  if (!fab || !panel || !messagesEl || !typingHost || !input || !sendBtn) return;

  const messageList = createMessageList(messagesEl);
  messageList.clear();
  const typing = createTypingIndicator(typingHost);

  const isInside = (node) => Boolean(node && root.contains(node));

  const chatWindow = createChatWindow({
    root,
    fab,
    panel,
    isInside,
    onOpen: () => {
      setTimeout(() => input.focus(), 280);
    },
    onClose: () => {},
  });

  let loading = false;

  function setLoading(next) {
    loading = next;
    sendBtn.disabled = next;
    input.disabled = next;
    if (next) typing.show();
    else typing.hide();
  }

  async function sendCurrentMessage() {
    const text = input.value.trim();
    if (!text || loading) return;

    document.getElementById("floating-chat-empty-hint")?.remove();

    input.value = "";
    messageList.appendUser(text);
    setLoading(true);

    try {
      const response = await fetch(askUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ question: text }),
      });

      let data;
      try {
        data = await response.json();
      } catch {
        messageList.appendError("Invalid response from server.");
        return;
      }

      const reply =
        typeof data.answer === "string"
          ? data.answer
          : data.detail != null
            ? JSON.stringify(data.detail)
            : "Request failed.";

      if (!response.ok) {
        messageList.appendError(reply);
        return;
      }

      messageList.appendAssistant(reply);
    } catch {
      messageList.appendError("Network error. Check your connection and try again.");
    } finally {
      setLoading(false);
    }
  }

  wireFloatingChatButton(fab, () => chatWindow.toggle());

  btnMinimize?.addEventListener("click", (e) => {
    e.stopPropagation();
    chatWindow.close();
  });

  btnClose?.addEventListener("click", (e) => {
    e.stopPropagation();
    messageList.clear();
    chatWindow.close();
  });

  sendBtn.addEventListener("click", (e) => {
    e.preventDefault();
    sendCurrentMessage();
  });

  input.addEventListener("keydown", (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      sendCurrentMessage();
    }
  });

  root.addEventListener("keydown", (e) => {
    if (e.key === "Escape" && chatWindow.isOpen()) {
      e.stopPropagation();
      chatWindow.close();
    }
  });
}
