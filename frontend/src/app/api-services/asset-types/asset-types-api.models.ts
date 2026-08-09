export interface AssetTypeDto {
  id: number;
  name: string;
  defaultSlaHours?: number | null;
  assetCount: number;
}

export interface SaveAssetTypeCommand {
  name: string;
  defaultSlaHours?: number | null;
}
