/** Mirrors the backend PageResult<T>. */
export interface PageResult<T> {
  items: T[];
  pageSize: number;
  currentPage: number;
  totalItems: number;
  totalPages: number;
}
