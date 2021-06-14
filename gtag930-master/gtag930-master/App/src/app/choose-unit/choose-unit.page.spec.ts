import {async, ComponentFixture, TestBed} from '@angular/core/testing';
import {IonicModule} from '@ionic/angular';

import {ChooseUnitPage} from './choose-unit.page';

describe('ChooseUnitPage', () => {
    let component: ChooseUnitPage;
    let fixture: ComponentFixture<ChooseUnitPage>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ChooseUnitPage],
            imports: [IonicModule.forRoot()]
        }).compileComponents();

        fixture = TestBed.createComponent(ChooseUnitPage);
        component = fixture.componentInstance;
        fixture.detectChanges();
    }));

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
