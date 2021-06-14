import {async, ComponentFixture, TestBed} from '@angular/core/testing';
import {IonicModule} from '@ionic/angular';

import {AddUnitPage} from './add-unit.page';

describe('AddUnitPage', () => {
    let component: AddUnitPage;
    let fixture: ComponentFixture<AddUnitPage>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [AddUnitPage],
            imports: [IonicModule.forRoot()]
        }).compileComponents();

        fixture = TestBed.createComponent(AddUnitPage);
        component = fixture.componentInstance;
        fixture.detectChanges();
    }));

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
