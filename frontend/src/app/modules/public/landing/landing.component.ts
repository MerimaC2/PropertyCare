import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthFacadeService } from '../../../core/services/auth-facade.service';

/** Custom-designed public landing page. */
@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss'],
  standalone: false
})
export class LandingComponent {
  readonly features = [
    {
      icon: 'report_problem',
      title: 'Report faults in seconds',
      text: 'Describe the problem, pick the building and equipment, set a priority - done.'
    },
    {
      icon: 'track_changes',
      title: 'Track every request',
      text: 'Filter your reports by status, priority, building and date, and always know what is happening.'
    },
    {
      icon: 'engineering',
      title: 'Smart triage & assignment',
      text: 'Dispatchers review incoming requests and assign the right technician with one click.'
    },
    {
      icon: 'insights',
      title: 'Clear SLA priorities',
      text: 'Every priority carries a service-level deadline so critical faults never wait.'
    }
  ];

  constructor(
    private authFacade: AuthFacadeService,
    private router: Router
  ) {}

  get isLoggedIn(): boolean {
    return this.authFacade.isLoggedIn();
  }

  onPrimaryAction(): void {
    if (this.isLoggedIn) {
      this.router.navigateByUrl(this.authFacade.homeUrlForCurrentUser());
    } else {
      this.router.navigate(['/login']);
    }
  }
}
