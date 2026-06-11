// Mirrors backend Lookups module DTOs.

export interface LookupItemDto {
  id: number;
  name: string;
}

export interface UnitLookupDto {
  id: number;
  buildingId: number;
  label: string;
}

export interface AssetLookupDto {
  id: number;
  unitId: number;
  name: string;
}

export interface RequestFormLookupsDto {
  buildings: LookupItemDto[];
  units: UnitLookupDto[];
  assets: AssetLookupDto[];
  priorities: LookupItemDto[];
  statuses: LookupItemDto[];
}

export interface TriageLookupsDto {
  technicians: LookupItemDto[];
  statuses: LookupItemDto[];
  priorities: LookupItemDto[];
  buildings: LookupItemDto[];
}
