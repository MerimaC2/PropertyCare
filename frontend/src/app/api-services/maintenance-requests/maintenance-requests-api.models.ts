import { PageRequest } from '../../core/models/paging/page-request';

// === COMMANDS ===

export interface CreateMaintenanceRequestCommand {
  title: string;
  description: string;
  buildingId: number;
  unitId?: number | null;
  assetId?: number | null;
  priorityId: number;
}

// === QUERIES ===

/** 5 filter parameters: status, priority, building, date from, date to. */
export interface ListMyMaintenanceRequestsQuery {
  paging: PageRequest;
  statusId?: number | null;
  priorityId?: number | null;
  buildingId?: number | null;
  dateFrom?: Date | null;
  dateTo?: Date | null;
}

// === DTOs ===

export interface MyMaintenanceRequestDto {
  id: number;
  title: string;
  buildingName: string;
  unitLabel?: string | null;
  assetName?: string | null;
  priorityName: string;
  priorityAbrv: string;
  statusName: string;
  statusAbrv: string;
  createdAtUtc: string;
}
