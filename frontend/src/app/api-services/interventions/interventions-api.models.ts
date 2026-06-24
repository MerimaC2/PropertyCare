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
  createdAtUtc: string;
  totalMinutes: number;
  logs: WorkLogDto[];
}

export interface AddWorkLogCommand {
  note: string;
  minutesSpent: number;
}
