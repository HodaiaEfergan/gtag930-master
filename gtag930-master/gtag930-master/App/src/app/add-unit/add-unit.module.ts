import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {IonicModule} from '@ionic/angular';

import {AddUnitPageRoutingModule} from './add-unit-routing.module';

import {AddUnitPage} from './add-unit.page';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        IonicModule,
        AddUnitPageRoutingModule
    ],
    declarations: [AddUnitPage]
})
export class AddUnitPageModule {
}
