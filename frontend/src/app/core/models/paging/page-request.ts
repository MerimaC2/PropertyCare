/** Mirrors the backend PageRequest. */
export interface PageRequest {
  page: number;
  pageSize: number;
}

export const DEFAULT_PAGE_SIZE = 10;
