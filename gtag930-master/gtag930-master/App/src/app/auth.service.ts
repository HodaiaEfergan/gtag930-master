import {environment} from './../environments/environment';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

@Injectable({
    providedIn: 'root',
})
export class AuthService {
    apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) {
    }

    logIn(model: any) {
        return this.http.post(this.apiUrl + 'appsignin', model, {
            withCredentials: true,
        });
    }

    silentLogin() {
        var model = {
            email: localStorage.getItem('email'),
            password: localStorage.getItem('password'),
        };
        return this.logIn(model);
    }

    logOut() {
        localStorage.removeItem('email');
        localStorage.removeItem('password');
    }
}
