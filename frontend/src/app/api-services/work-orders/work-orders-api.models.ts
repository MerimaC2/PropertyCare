// === COMMANDS ===

export interface AssignWorkOrderCommand {
  requestId: number;
  assignedToUserId: number;
  note?: string | null;
}
