import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing-module';
import { AppComponent } from './app';
import { AboutCompanyComponent } from './components/about-company/about-company';

@NgModule({
  imports: [
    BrowserModule,
    AppRoutingModule,
    AppComponent,
    AboutCompanyComponent
  ],
  providers: [
    provideBrowserGlobalErrorListeners()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
