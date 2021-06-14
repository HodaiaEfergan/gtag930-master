import {Component, OnInit} from '@angular/core';
import {environment} from 'src/environments/environment';
import {HttpClient} from '@angular/common/http';
import {AlertController, ModalController} from '@ionic/angular';

@Component({
    selector: 'app-add-unit',
    templateUrl: './add-unit.page.html',
    styleUrls: ['./add-unit.page.scss'],
})
export class AddUnitPage implements OnInit {
    public unitId: string;
    public unitName: string;

    constructor(private http: HttpClient, private alertController: AlertController, private modalController: ModalController) {
    }

    ngOnInit() {
    }

    public async addUnit() {
        if (!this.unitId || !this.unitName) {
            const alert = await this.alertController.create({
                header: 'Not all fields are filled',
                message: 'To add a new unit, you must fill unit ID and Unit Name',
                buttons: ['OK'],
            });

            await alert.present();
            return;
        }
        this.http.post(environment.apiUrl + 'units', {Id: this.unitId, Name: this.unitName}, {
            withCredentials: true,
        }).subscribe(ret => {
            this.modalController.dismiss({id: this.unitId});
        });
    }

    public close() {
        this.modalController.dismiss();
    }
}
