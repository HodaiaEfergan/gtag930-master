import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import {RouterModule} from '@angular/router';

import {AppComponent} from './app.component';
import {NavMenuComponent} from './nav-menu/nav-menu.component';
import {HomeComponent} from './home/home.component';
import {ApiAuthorizationModule} from 'src/api-authorization/api-authorization.module';
import {AuthorizeGuard} from 'src/api-authorization/authorize.guard';
import {AuthorizeInterceptor} from 'src/api-authorization/authorize.interceptor';
import {IndexComponent} from './admin/index/index.component';
import {UsersComponent} from './admin/users/users.component';
import {UnitsComponent} from './admin/units/units.component';
import {AdminComponent} from './admin/admin.component';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {GoogleMapsModule} from '@angular/google-maps';
import {NgMultiSelectDropDownModule} from 'ng-multiselect-dropdown';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    IndexComponent,
    UsersComponent,
    UnitsComponent,
    AdminComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    FormsModule,
    ApiAuthorizationModule,
    RouterModule.forRoot([
      {path: '', component: HomeComponent, pathMatch: 'full', canActivate: [AuthorizeGuard]},
      {
        path: 'admin', component: AdminComponent, canActivate: [AuthorizeGuard], children: [
          {path: 'index', component: IndexComponent},
          {path: 'users', component: UsersComponent},
          {path: 'units', component: UnitsComponent}
        ]
      }
    ]),
    NgbModule,
    ReactiveFormsModule,
    GoogleMapsModule,
    NgMultiSelectDropDownModule,
    BrowserAnimationsModule,
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true},
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
