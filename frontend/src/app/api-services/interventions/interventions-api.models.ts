export interface WorkLogDto {
  id: number;
  note: string;
  minutesSpent: number;
  createdAtUtc: string;
}

export interface InterventionDto {
  workOrderId: number;
  requestTitle: string;
  buildingName: string;
  statusName: string;
  /** True once the order is completed or cancelled, so no further work can be logged. */
  statusIsTerminal: boolean;
  createdAtUtc: string;
  totalMinutes: number;
  logs: WorkLogDto[];
}

export interface AddWorkLogCommand {
  note: string;
  minutesSpent: number;
}
