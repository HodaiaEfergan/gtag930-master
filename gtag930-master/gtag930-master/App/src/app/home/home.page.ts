import {AuthService} from './../auth.service';
import {Component} from '@angular/core';
import {Router} from '@angular/router';

@Component({
    selector: 'app-home',
    templateUrl: 'home.page.html',
    styleUrls: ['home.page.scss'],
})
export class HomePage {
    constructor(private router: Router, private authService: AuthService) {
        this.login();
    }

    login() {
        this.authService.silentLogin().subscribe(
            (res) => {
                this.router.navigate(['main']);
            },
            (err) => {
                this.router.navigate(['login']);
            }
        );
    }
}
