import { HttpParams } from '@angular/common/http';

/**
 * Converts an object into HttpParams, supporting nested objects and arrays.
 * Examples:
 *   { paging: { page: 1, pageSize: 20 } } -> ?paging.page=1&paging.pageSize=20
 *   { ids: [1, 2] }                       -> ?ids=1&ids=2
 *   { search: null }                      -> (skipped)
 */
export function buildHttpParams(obj: Record<string, unknown>, prefix = ''): HttpParams {
  let params = new HttpParams();

  if (obj === undefined || obj === null) {
    return params;
  }

  Object.entries(obj).forEach(([key, value]) => {
    const fullKey = prefix ? `${prefix}.${key}` : key;

    if (value === null || value === undefined) {
      return;
    }

    if (typeof value === 'string' && value.trim() === '') {
      return;
    }

    if (Array.isArray(value)) {
      value.forEach(item => {
        if (item !== null && item !== undefined) {
          params = params.append(fullKey, String(item));
        }
      });
      return;
    }

    if (value instanceof Date) {
      params = params.set(fullKey, value.toISOString());
      return;
    }

    if (typeof value === 'object') {
      const nested = buildHttpParams(value as Record<string, unknown>, fullKey);
      nested.keys().forEach(nestedKey => {
        nested.getAll(nestedKey)?.forEach(nestedValue => {
          params = params.append(nestedKey, nestedValue);
        });
      });
      return;
    }

    params = params.set(fullKey, String(value));
  });

  return params;
}
