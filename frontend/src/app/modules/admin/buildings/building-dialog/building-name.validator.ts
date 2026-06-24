import { AbstractControl, AsyncValidatorFn, ValidationErrors } from '@angular/forms';
import { Observable, of, timer } from 'rxjs';
import { catchError, first, map, switchMap } from 'rxjs/operators';
import { BuildingsApiService } from '../../../../api-services/buildings/buildings-api.service';

/**
 * Async validator: debounces, then asks the backend whether the building name is already taken.
 * Sets a `nameTaken` error when it is. Ignores `excludeId` (the building being edited).
 */
export function buildingNameTakenValidator(
  api: BuildingsApiService,
  excludeId?: number
): AsyncValidatorFn {
  return (control: AbstractControl): Observable<ValidationErrors | null> => {
    const value = (control.value ?? '').toString().trim();
    if (!value) {
      return of(null);
    }
    return timer(400).pipe(
      switchMap(() => api.nameExists(value, excludeId)),
      map(exists => (exists ? { nameTaken: true } : null)),
      catchError(() => of(null)),
      first()
    );
  };
}
