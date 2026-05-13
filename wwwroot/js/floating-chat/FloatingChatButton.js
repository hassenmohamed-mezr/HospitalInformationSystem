/**
 * FAB interactions (pulse handled in CSS).
 */
export function wireFloatingChatButton(fab, onActivate) {
  const onClick = (e) => {
    e.preventDefault();
    e.stopPropagation();
    onActivate();
  };
  fab.addEventListener("click", onClick);
  return () => fab.removeEventListener("click", onClick);
}
