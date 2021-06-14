import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';

import {ChooseUnitPage} from './choose-unit.page';

const routes: Routes = [
    {
        path: '',
        component: ChooseUnitPage
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ChooseUnitPageRoutingModule {
}
