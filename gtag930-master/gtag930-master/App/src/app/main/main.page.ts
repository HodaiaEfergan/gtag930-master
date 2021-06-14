import {UnitService} from './../unit.service';
import {ChangeDetectorRef, Component, OnDestroy} from '@angular/core';
import {Geolocation} from '@ionic-native/geolocation/ngx';
import {App, CameraResultType, CameraSource, Plugins} from '@capacitor/core';
import {BluetoothSerial} from '@ionic-native/bluetooth-serial/ngx';
import {Unit} from './../unit';
import {AlertController, ModalController, Platform} from '@ionic/angular';
import {AuthService} from '../auth.service';
import {Router} from '@angular/router';
import {AddUnitPage} from '../add-unit/add-unit.page';
import {ChooseUnitPage} from '../choose-unit/choose-unit.page';
import {SubSink} from 'subsink';

@Component({
    selector: 'app-main',
    templateUrl: './main.page.html',
    styleUrls: ['./main.page.scss'],
})
export class MainPage implements OnDestroy {
    image64: any = null;
    selectedImage: string = null;
    isLocated = false;
    coordinates: Coordinates = null;
    isConnectedToUnit: boolean;
    btDeviceToConnect: any = null;
    selectedUnit: number;
    description: string;
    units: Unit[] = [];
    tags = [];
    bluetoothTagStr = '';
    state = 'None';
    subs = new SubSink();

    constructor(
        private geolocation: Geolocation,
        private bluetoothSerial: BluetoothSerial,
        private unitService: UnitService,
        private alertController: AlertController,
        private authService: AuthService,
        private router: Router,
        private modalController: ModalController,
        private cdr: ChangeDetectorRef,
        private platform: Platform
    ) {
    }

    ionViewWillEnter() {
        this.subs.sink = this.platform.backButton.subscribe(async () => {
            App.exitApp();
        });
        this.resetControls();
        this.locateGPS();
        this.initBluetooth();
        this.fetchUnits();
    }

    ionViewDidLeave() {
        this.subs.unsubscribe();
    }

    async save() {
        const model = {
            unitId: null,
            tagIds: this.tags,
            description: this.description,
            ImageJpegBase64: this.image64,
            gpsLatitude: null,
            gpsLongitude: null,
            state: this.state
        };
        if (this.selectedUnit) {
            model.unitId = this.selectedUnit
        }
        if (this.coordinates) {
            model.gpsLatitude = this.coordinates.latitude;
            model.gpsLongitude = this.coordinates.longitude;
        }
        await this.unitService.saveUnitInfo(model);
        this.resetControls();
    }

    resetControls() {
        this.tags = [];
        this.image64 = null;
        this.selectedImage = null;
        this.description = '';
    }

    fetchUnits() {
        console.log('refreshing units...');
        this.subs.sink = this.unitService.getAllUnitIds().subscribe(
            (res) => {
                this.units = res;
                this.cdr.detectChanges();
            },
            (err) => {
                console.log(err);
            }
        );
    }

    takePicture() {
        Plugins.Camera.getPhoto({
            quality: 75,
            source: CameraSource.Camera,
            correctOrientation: true,
            resultType: CameraResultType.Base64,
        })
            .then((image) => {
                this.image64 = image.base64String;
                this.selectedImage = 'data:image/jpeg;base64,' + image.base64String;
            })
            .catch((err) => {
            });
    }

    addTag(input) {
        const tag = input.value;
        if (tag !== '') {
            if (this.tags.indexOf(tag) === -1) {
                this.tags.push(tag);
            }
        }
        input.value = '';
    }

    removeTag(tag) {
        const index = this.tags.indexOf(tag);
        this.tags.splice(index, 1);
        this.cdr.detectChanges();
    }

    selectedUnitValue(event) {
        console.log(event.target.value);
    }

    chooseUnit() {
        this.modalController.create({component: ChooseUnitPage}).then((modal) => {
            modal.present();
            modal.onDidDismiss().then((modalRes) => {
                console.log('choose unit modal closed. ', modalRes);
                const device = modalRes.data;
                if (device) {
                    if (this.btDeviceToConnect) {
                        // disconnect connected device first
                        this.isConnectedToUnit = false;
                        this.bluetoothSerial.disconnect();
                    }
                    this.btDeviceToConnect = device;
                    this.cdr.detectChanges();
                    this.connectToDevice(device);
                }
            });
        });
    }

    connectToDevice(device) {
        this.subs.sink = this.bluetoothSerial.connect(device.address).subscribe(
            (res) => {
                console.log('bt connection result: ', res);
                this.isConnectedToUnit = true;
                this.cdr.detectChanges();
                this.subs.sink = this.bluetoothSerial.subscribeRawData().subscribe(
                    (data) => {
                        const results = this.buf2hex(data).toString();
                        console.log('incoming bt data: ', results);
                        this.bluetoothTagStr = this.bluetoothTagStr.concat(results);
                        console.log('current bluetooth string: ' + this.bluetoothTagStr);

                        if (this.bluetoothTagStr.length > 28) {
                            console.error(
                                'invalid tag transmission, tag transmission is 14 bytes long, and must start with 0x02'
                            );
                            this.bluetoothTagStr = '';
                            return;
                        } else if (this.bluetoothTagStr.length === 28) {
                            if (!this.bluetoothTagStr.startsWith('02')) {
                                console.error('invalid tag transmission, must start with 0x02');
                                this.bluetoothTagStr = '';
                                return;
                            }
                            // omit first byte and last byte
                            const hexTag = this.bluetoothTagStr.substring(2, 26);
                            const ascii = this.hex_to_ascii(hexTag);
                            console.log('tag ascii: ' + ascii);
                            if (this.tags.indexOf(ascii) === -1) {
                                this.tags.push(ascii);
                                this.cdr.detectChanges();
                            }
                            this.bluetoothTagStr = '';
                        }
                    },
                    (err) => {
                        console.log('bt error while subscribing to raw data: ', err);
                    }
                );
            },
            (err) => {
                console.log('bt error while connecting: ', err);
                this.isConnectedToUnit = false;
                this.cdr.detectChanges();
            }
        );
    }

    initBluetooth() {
        if (!this.bluetoothSerial.isEnabled()) {
            console.log('enabling bluetooth');
            this.bluetoothSerial.enable().then((res) => {
                console.log('bt enable result: ' + res);
            });
        }
    }

    hex_to_ascii(str1) {
        const hex = str1.toString();
        let str = '';
        for (let n = 0; n < hex.length; n += 2) {
            str += String.fromCharCode(parseInt(hex.substr(n, 2), 16));
        }
        return str;
    }

    buf2hex(buffer) {
        // buffer is an ArrayBuffer
        return Array.prototype.map
            .call(new Uint8Array(buffer), (x) => ('00' + x.toString(16)).slice(-2))
            .join('');
    }

    locateGPS() {
        const watch = this.geolocation.watchPosition();
        this.subs.sink = watch.subscribe(
            (data) => {
                console.log('GPS located.', data);
                this.isLocated = true;
                this.coordinates = data.coords;
            },
            (err) => {
                this.isLocated = false;
            }
        );
    }

    logout() {
        this.authService.logOut();
        this.router.navigate(['login']);
    }

    async openAddUnitModal() {
        const modal = await this.modalController.create({component: AddUnitPage});
        await modal.present();
        modal.onDidDismiss().then((data) => {
            console.log('add unit modal closed. ', data);
            this.fetchUnits();
        });
    }

    ngOnDestroy() {
        this.bluetoothSerial.disconnect();
    }
}
