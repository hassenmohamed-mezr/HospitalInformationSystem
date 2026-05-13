/**
 * Panel open/close transitions and outside-click handling.
 */
export function createChatWindow({
  root,
  fab,
  panel,
  isInside,
  onOpen,
  onClose,
}) {
  let open = false;

  function setOpen(next) {
    if (open === next) return;
    open = next;
    fab.setAttribute("aria-expanded", open ? "true" : "false");
    panel.classList.toggle("is-open", open);
    panel.setAttribute("aria-hidden", open ? "false" : "true");
    if (open) onOpen?.();
    else onClose?.();
  }

  function handleDocPointerDown(ev) {
    if (!open) return;
    const t = ev.target;
    if (isInside(t)) return;
    setOpen(false);
  }

  document.addEventListener("pointerdown", handleDocPointerDown, true);

  return {
    isOpen: () => open,
    open() {
      setOpen(true);
    },
    close() {
      setOpen(false);
    },
    toggle() {
      setOpen(!open);
    },
    dispose() {
      document.removeEventListener("pointerdown", handleDocPointerDown, true);
    },
  };
}
