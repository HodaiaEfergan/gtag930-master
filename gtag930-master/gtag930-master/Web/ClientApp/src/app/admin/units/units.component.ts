import {Component, ElementRef, Inject, OnInit, ViewChild} from '@angular/core';
import {Observable} from 'rxjs';
import {IUser} from '../../models/iuser';
import {IUnitInfo} from '../../models/IUnitInfo';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {FormBuilder, FormGroup} from '@angular/forms';

@Component({
  selector: 'app-units',
  templateUrl: './units.component.html',
  styleUrls: ['./units.component.css']
})
export class UnitsComponent implements OnInit {
  public users: Observable<IUser[]>;
  public units: Observable<IUnitInfo[]>;
  @ViewChild('infoModal') infoModal: ElementRef;
  infoModalTitle: string;
  infoModalBody: string;
  editUnitForm: FormGroup;
  newUnitForm: FormGroup;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string,
              private modalService: NgbModal, private fb: FormBuilder) {
  }

  ngOnInit() {
    this.refreshData();

    this.editUnitForm = this.fb.group({
      id: [''],
      name: [''],
      hexRgbColor: ['']
    });

    this.newUnitForm = this.fb.group({
      id: [''],
      name: [''],
      hexRgbColor: ['']
    });
  }

  deleteUnit(unit: IUnitInfo) {
    this.http.delete<string>(this.baseUrl + 'units/' + unit.id).subscribe((res) => {
        this.openInfoModal('Unit Deleted');
        this.refreshData();
      }, (err: HttpErrorResponse) => {
        this.openInfoModal(err.error);
        console.error(err.error);
      }
    );
  }

  isUnitAttachedToUser(unit: IUnitInfo, user: IUser) {
    return unit.assignedUsers.findIndex(u => u.id === user.id) !== -1;
  }

  private openInfoModal(message: string) {
    this.infoModalTitle = message;
    this.infoModalBody = message;
    this.modalService.open(this.infoModal);
  }

  private refreshData() {
    this.users = this.http.get<IUser[]>(this.baseUrl + 'users');
    this.units = this.http.get<IUnitInfo[]>(this.baseUrl + 'units?onlyOwn=false');
  }

  editUnit(editUnitModal: any, unit: IUnitInfo) {
    this.modalService.open(editUnitModal, {
      centered: true,
      backdrop: 'static'
    });

    this.editUnitForm.patchValue({
      id: unit.id,
      name: unit.name,
      hexRgbColor: unit.hexRgbColor
    });
  }

  editUnitSubmit() {
    this.modalService.dismissAll();
    const editedUnit: IUnitInfo = this.editUnitForm.getRawValue();
    // because the edited unit is missing the assignedUsers property,
    // this will remove all assigned users. (bug or a feature?)
    this.doneUserEdit(editedUnit);
  }

  UnitAttachedToUserChange(cbEvent: any, unit: IUnitInfo, user: IUser) {
    const isChecked: boolean = cbEvent.target.checked;

    if (isChecked) {
      unit.assignedUsers.push(user);
    } else {
      unit.assignedUsers = unit.assignedUsers.filter(u => u.id !== user.id);
    }

    this.doneUserEdit(unit, false);
  }

  private doneUserEdit(unit: IUnitInfo, openConfirmationModal: boolean = true) {
    this.http.put<string>(this.baseUrl + 'units/' + unit.id, unit).subscribe((res) => {
        if (openConfirmationModal) {
          this.openInfoModal('Unit Edited');
        }
        this.refreshData();
      }, (err: HttpErrorResponse) => {
        this.openInfoModal(err.error);
        console.error(err.error);
      }
    );
  }

  createNewUnit() {
    const newUnit: IUnitInfo = this.newUnitForm.getRawValue();

    if (!newUnit.id) {
      this.openInfoModal('Missing ID. required field');
      return;
    }
    if (!newUnit.name) {
      this.openInfoModal('Missing Name. required field');
      return;
    }

    this.http.post<string>(this.baseUrl + 'units/', newUnit).subscribe((res) => {
        this.openInfoModal('New Unit Created');
        this.newUnitForm.reset();
        this.refreshData();
      }, (err: HttpErrorResponse) => {
        this.openInfoModal(err.error);
        console.error(err.error);
      }
    );
  }
}
