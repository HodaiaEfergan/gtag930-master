import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {IonicModule} from '@ionic/angular';

import {ChooseUnitPageRoutingModule} from './choose-unit-routing.module';

import {ChooseUnitPage} from './choose-unit.page';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        IonicModule,
        ChooseUnitPageRoutingModule
    ],
    declarations: [ChooseUnitPage]
})
export class ChooseUnitPageModule {
}
