import {Component, OnInit} from '@angular/core';
import {BluetoothSerial} from '@ionic-native/bluetooth-serial/ngx';
import {ModalController} from '@ionic/angular';

@Component({
    selector: 'app-choose-unit',
    templateUrl: './choose-unit.page.html',
    styleUrls: ['./choose-unit.page.scss'],
})
export class ChooseUnitPage implements OnInit {

    private bluetoothDevices: any[];

    constructor(private bluetoothSerial: BluetoothSerial, private modalController: ModalController) {
    }

    ngOnInit() {
        this.bluetoothSerial.list().then((devices) => {
            console.log('bt list: ', devices);
            this.bluetoothDevices = devices;
        });
    }

    deviceClick(device) {
        this.modalController.dismiss(device);
    }

    closeModal() {
        this.modalController.dismiss();
    }
}
