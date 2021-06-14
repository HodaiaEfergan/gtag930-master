import {Component, ElementRef, Inject, OnInit, ViewChild} from '@angular/core';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {IUser} from '../../models/iuser';
import {Observable} from 'rxjs';
import {IRole} from '../../models/irole';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {FormBuilder, FormGroup} from '@angular/forms';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
  public users: Observable<IUser[]>;
  infoModalTitle: string;
  infoModalBody: string;
  @ViewChild('infoModal') infoModal: ElementRef;
  editUserForm: FormGroup;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string,
              private modalService: NgbModal, private fb: FormBuilder) {
  }

  ngOnInit(): void {
    this.refreshUsers();

    this.editUserForm = this.fb.group({
      id: [''],
      role: ['Regular'],
      isLocked: [false]
    });
  }

  private refreshUsers() {
    this.users = this.http.get<IUser[]>(this.baseUrl + 'users');
  }

  getUserRole(roles: IRole[]) {
    if (roles.findIndex(x => x.name === 'Admin') !== -1) {
      return 'Admin';
    } else {
      return 'Regular';
    }
  }

  editUser(targetModal: any, user: IUser) {
    this.modalService.open(targetModal, {
      centered: true,
      backdrop: 'static'
    });

    this.editUserForm.patchValue({
      id: user.id,
      role: this.getUserRole(user.roles),
      isLocked: user.isLocked
    });
  }

  editUserSubmit() {
    this.modalService.dismissAll();
    const editedUser: IUser = this.editUserForm.getRawValue();

    this.http.put<string>(this.baseUrl + 'users/' + editedUser.id, editedUser).subscribe((res) => {
        this.openInfoModal('User Edited');
        this.refreshUsers();
      }, (err: HttpErrorResponse) => {
        this.openInfoModal(err.error);
        console.error(err.error);
      }
    );
  }

  deleteUser(user: IUser) {
    let deleteUser = confirm('Are you sure you want to delete the user?');
    if (!deleteUser) {
      return;
    }

    this.http.delete<string>(this.baseUrl + 'users/' + user.id).subscribe((res) => {
        this.openInfoModal('User Deleted');
        this.refreshUsers();
      }, (err: HttpErrorResponse) => {
        this.openInfoModal(err.error);
        console.error(err.error);
      }
    );
  }

  private openInfoModal(message: string) {
    this.infoModalTitle = message;
    this.infoModalBody = message;
    this.modalService.open(this.infoModal);
  }
}
