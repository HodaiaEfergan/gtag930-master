import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {RouteReuseStrategy} from '@angular/router';

import {IonicModule, IonicRouteStrategy} from '@ionic/angular';
import {SplashScreen} from '@ionic-native/splash-screen/ngx';
import {StatusBar} from '@ionic-native/status-bar/ngx';

import {AppComponent} from './app.component';
import {AppRoutingModule} from './app-routing.module';
import {HttpClientModule} from '@angular/common/http';
import {Geolocation} from '@ionic-native/geolocation/ngx';
import {BluetoothSerial} from '@ionic-native/bluetooth-serial/ngx';

@NgModule({
    declarations: [AppComponent],
    entryComponents: [],
    imports: [
        HttpClientModule,
        BrowserModule,
        IonicModule.forRoot(),
        AppRoutingModule,
    ],
    providers: [
        BluetoothSerial,
        Geolocation,
        StatusBar,
        SplashScreen,
        {provide: RouteReuseStrategy, useClass: IonicRouteStrategy},
    ],
    bootstrap: [AppComponent],
})
export class AppModule {
}
