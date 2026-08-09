export interface CountByLabelDto {
  label: string;
  count: number;
}

export interface DashboardStatsDto {
  totalRequests: number;
  byStatus: CountByLabelDto[];
  byPriority: CountByLabelDto[];
}
