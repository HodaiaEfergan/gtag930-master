import {environment} from './../environments/environment';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Unit} from './unit';
import {Plugins} from '@capacitor/core';
import {ToastController} from '@ionic/angular';

const {Network} = Plugins;
const {Storage} = Plugins;

@Injectable({
    providedIn: 'root',
})
export class UnitService {
    unitScansKey = 'unitScans';
    apiUrl = environment.apiUrl;
    isSavingUnitScans = false;

    constructor(private http: HttpClient, public toastController: ToastController) {
        this.resetSavedUnitScansStorage().then(r => {
            setInterval(async () => {
                if (this.isSavingUnitScans) {
                    return;
                }

                this.isSavingUnitScans = true;
                await this.saveUnitScans();
                this.isSavingUnitScans = false;
            }, 1000);
        });
    }

    private async saveUnitScans() {
        const savedUnitScans = await this.getSavedUnitScans();
        if (savedUnitScans.length === 0) {
            return;
        }

        const networkStatus = await Network.getStatus();
        if (!networkStatus.connected) {
            return;
        }

        for (const scanModel of savedUnitScans) {
            try {
                await this.http.post(this.apiUrl + 'unitscans', scanModel, {
                    withCredentials: true,
                }).toPromise();

                const toast = await this.toastController.create({
                    message: 'Unit scan saved!',
                    duration: 2000
                });
                await toast.present();

                await this.resetSavedUnitScansStorage();
            } catch (err) {
                const errToast = await this.toastController.create({
                    message: 'Error while trying to save scan',
                    duration: 2000
                });
                await errToast.present();
            }
        }
    }

    getAllUnitIds() {
        return this.http.get<Unit[]>(this.apiUrl + 'units' + '?onlyown=true', {
            withCredentials: true,
        });
    }

    async saveUnitInfo(model: any) {
        const savedUnitScans = await this.getSavedUnitScans();
        savedUnitScans.push(model);
        await Storage.set({
            key: this.unitScansKey,
            value: JSON.stringify(savedUnitScans)
        });
    }

    private async getSavedUnitScans() {
        const ret = await Storage.get({key: this.unitScansKey});
        return JSON.parse(ret.value);
    }

    private resetSavedUnitScansStorage() {
        return Storage.set({
            key: this.unitScansKey,
            value: JSON.stringify([])
        });
    }
}
