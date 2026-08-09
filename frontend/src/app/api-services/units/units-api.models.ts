export interface UnitDto {
  id: number;
  buildingId: number;
  label: string;
  assetCount: number;
}

export interface CreateUnitCommand {
  buildingId: number;
  label: string;
}

export interface UpdateUnitCommand {
  label: string;
}
