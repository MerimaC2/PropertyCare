import { HttpParams } from '@angular/common/http';

/**
 * Formats a Date as the calendar day it represents, in the user's own timezone.
 *
 * Every Date this app puts in a query string comes from a datepicker, so it is a day and not a
 * moment. toISOString would first convert local midnight to UTC, which east of Greenwich lands on
 * the previous day - the filter then quietly searched the wrong day.
 */
const toCalendarDate = (value: Date): string => {
  const month = `${value.getMonth() + 1}`.padStart(2, '0');
  const day = `${value.getDate()}`.padStart(2, '0');
  return `${value.getFullYear()}-${month}-${day}`;
};

/**
 * Converts an object into HttpParams, supporting nested objects and arrays.
 * Examples:
 *   { paging: { page: 1, pageSize: 20 } } -> ?paging.page=1&paging.pageSize=20
 *   { ids: [1, 2] }                       -> ?ids=1&ids=2
 *   { from: new Date(2026, 8, 6) }        -> ?from=2026-09-06
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
      params = params.set(fullKey, toCalendarDate(value));
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
