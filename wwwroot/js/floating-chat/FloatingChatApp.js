import { createTypingIndicator } from "./TypingIndicator.js";
import { createMessageList } from "./MessageList.js";
import { createChatWindow } from "./ChatWindow.js";
import { wireFloatingChatButton } from "./FloatingChatButton.js";

/**
 * Wires the floating assistant: messages, API, loading state, minimize vs close.
 */
export function initFloatingChat(root) {
  const askUrl = root.dataset.chatAskUrl;
  const endpoint = /\/Chat\/Ask$/i.test(askUrl || "") ? askUrl : "/Chat/Ask";
  if (!endpoint) return;

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
      const payload = { question: text };
      console.debug("[FloatingChat] POST URL:", endpoint);
      console.debug("[FloatingChat] Request payload:", payload);

      const response = await fetch(endpoint, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      const rawBody = await response.text();
      console.debug("[FloatingChat] HTTP status:", response.status);
      console.debug("[FloatingChat] Raw response:", rawBody);

      if (!rawBody || !rawBody.trim()) {
        messageList.appendError("Empty response from assistant.");
        return;
      }

      let data = null;
      try {
        data = JSON.parse(rawBody);
      } catch {
        messageList.appendError("Invalid response from server.");
        return;
      }
      console.debug("[FloatingChat] Parsed response payload:", data);

      const reply =
        typeof data?.answer === "string"
          ? data.answer
          : typeof data?.Answer === "string"
            ? data.Answer
            : data?.detail != null
            ? JSON.stringify(data.detail)
            : "";

      if (!response.ok) {
        messageList.appendError(reply || `Request failed (${response.status}).`);
        return;
      }

      if (!reply.trim()) {
        messageList.appendError("Assistant returned an empty answer.");
        return;
      }

      messageList.appendAssistant(reply);
    } catch (err) {
      console.error("[FloatingChat] Request failed:", err);
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
