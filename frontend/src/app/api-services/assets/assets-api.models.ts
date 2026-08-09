export interface AssetDto {
  id: number;
  unitId: number;
  name: string;
  assetTypeId: number;
  assetTypeName: string;
}

export interface CreateAssetCommand {
  unitId: number;
  name: string;
  assetTypeId: number;
}

export interface UpdateAssetCommand {
  name: string;
  assetTypeId: number;
}
