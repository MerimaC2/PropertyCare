import { PageRequest } from '../../core/models/paging/page-request';

export interface BuildingDto {
  id: number;
  name: string;
  address?: string | null;
  buildingTypeId: number;
  buildingTypeName: string;
  latitude?: number | null;
  longitude?: number | null;
  unitCount: number;
}

export interface ListBuildingsQuery {
  paging: PageRequest;
  search?: string | null;
}

/** Body for both create and update (the id travels in the URL for update). */
export interface SaveBuildingCommand {
  name: string;
  address?: string | null;
  buildingTypeId: number;
  latitude?: number | null;
  longitude?: number | null;
}
