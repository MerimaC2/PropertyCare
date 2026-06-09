import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared-module';
import { PublicRoutingModule } from './public-routing-module';
import { LandingComponent } from './landing/landing.component';
import { LoginComponent } from './login/login.component';

@NgModule({
  declarations: [LandingComponent, LoginComponent],
  imports: [SharedModule, PublicRoutingModule]
})
export class PublicModule {}
