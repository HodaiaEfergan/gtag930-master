import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';

import {AddUnitPage} from './add-unit.page';

const routes: Routes = [
    {
        path: '',
        component: AddUnitPage
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class AddUnitPageRoutingModule {
}
