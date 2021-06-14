import {Router} from '@angular/router';
import {AuthService} from './../auth.service';
import {Component, OnInit} from '@angular/core';

@Component({
    selector: 'app-login',
    templateUrl: './login.page.html',
    styleUrls: ['./login.page.scss'],
})
export class LoginPage implements OnInit {
    model: any = {};
    error: boolean;

    constructor(private authService: AuthService, private router: Router) {
    }

    ngOnInit() {
    }

    login(model?: any) {
        this.authService.logIn(this.model).subscribe(
            (res) => {
                if (this.model.rememberMe) {
                    localStorage.setItem('email', this.model.email);
                    localStorage.setItem('password', this.model.password);
                }
                this.router.navigate(['main']);
            },
            (err) => {
                localStorage.clear();
                console.log(err);
                this.error = true;
            }
        );
    }
}
