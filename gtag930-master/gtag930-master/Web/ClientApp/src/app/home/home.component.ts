import {Component, ElementRef, Inject, OnDestroy, OnInit, TemplateRef, ViewChild} from '@angular/core';
import {iif, interval, Observable} from 'rxjs';
import {IUnitInfo} from '../models/IUnitInfo';
import {HttpClient} from '@angular/common/http';
import {IUser} from '../models/iuser';
import {NgbDateStruct, NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {SubSink} from 'subsink';
import {IUnitScan} from '../models/IUnitScan';
import * as moment from 'moment';
import {GoogleMap, MapInfoWindow, MapMarker} from '@angular/google-maps';
import {ITagScan} from '../models/ITagScan';
import {ExcelService} from '../excel.service';
import flat from 'flat';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private modalService: NgbModal,
              private excelService: ExcelService) {
  }

  private subs = new SubSink();
  public units: IUnitInfo[] = [];
  public selectedUnits: IUnitInfo[] = [];
  public fromDate: NgbDateStruct;
  public fromTime = {hour: 0, minute: 0};
  public toDate: NgbDateStruct;
  public toTime = {hour: 23, minute: 59};
  public continuouslyUpdateData = true;
  public latestTagData = true;
  public unitScans: IUnitScan[] = [];
  @ViewChild(GoogleMap, {static: false}) googleMap: GoogleMap;
  public imageJpegBase64: string;
  public tagsFilter: string;
  public stateFilter = 'all';
  @ViewChild(MapInfoWindow, {static: false}) infoWindow: MapInfoWindow;
  infoContent = '';

  ngOnInit(): void {
    this.subs.sink = this.http.get<IUnitInfo[]>(this.baseUrl + 'units?onlyOwn=true').subscribe((units: IUnitInfo[]) => {
      this.units = units;
      // select all units
      this.selectedUnits = units;

      // after getting the units, its time to refresh data
      this.refreshData();
    });
    this.subs.sink = interval(1000 * 60).subscribe(seconds => {
      if (this.continuouslyUpdateData) {
        this.refreshData();
      }
    });
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  refreshData() {
    let scanUrl = this.baseUrl + 'unitscans?latestTagData=' + this.latestTagData;
    if (this.selectedUnits.length > 0) {
      scanUrl += '&' + this.getSelectedUnitIdsToQueryParams('unitIds');
    }
    if (this.fromDate) {
      const fromDate = new Date(this.fromDate.year, this.fromDate.month - 1,
        this.fromDate.day, this.fromTime.hour, this.fromTime.minute, 0);
      scanUrl += '&from=' + fromDate.toISOString();
    }
    if (this.toDate) {
      const toDate = new Date(this.toDate.year, this.toDate.month - 1, this.toDate.day, this.toTime.hour, this.toTime.minute, 59);
      scanUrl += '&to=' + toDate.toISOString();
    }
    if (this.tagsFilter) {
      scanUrl += '&' + this.getSelectedTagIdsToQueryParams('tagIds');
    }
    if (this.stateFilter !== 'all') {
      scanUrl += '&state=' + this.stateFilter;
    }
    this.http.get(scanUrl).subscribe((res: IUnitScan[]) => {
      console.table(res);
      const isChanged: boolean = JSON.stringify(res) !== JSON.stringify(this.unitScans);
      this.unitScans = res;

      if (isChanged) {
        this.autoCenterAndAutoZoomMap();
      }
    });
    console.log('refreshing data...');
  }

  private autoCenterAndAutoZoomMap() {
    const bounds = new google.maps.LatLngBounds();
    this.getUnitScansWithGps().forEach(unitScan => {
      bounds.extend(this.getGoogleMapsLatLngForUnitScan(unitScan));
    });

    this.googleMap.fitBounds(bounds);
  }

  private getSelectedUnitIdsToQueryParams(key: string) {
    let str = '';
    this.selectedUnits.forEach((unit) => {
      if (str !== '') {
        str += '&';
      }
      str += key + '=' + encodeURIComponent(unit.id);
    });
    return str;
  }

  private getSelectedTagIdsToQueryParams(key: string) {
    const tagsArray: string[] = this.tagsFilter.split(',');
    let str = '';
    tagsArray.forEach((tag) => {
      if (str !== '') {
        str += '&';
      }
      str += key + '=' + encodeURIComponent(tag);
    });
    return str;
  }

  unitSelectionChanged() {
    this.refreshData();
  }

  continuouslyUpdateTagDataChange(newValue: boolean) {
    this.refreshData();
  }

  fromChanged(newValue: NgbDateStruct) {
    this.refreshData();
  }

  fromTimeChanged(newValue: NgbDateStruct) {
    this.refreshData();
  }

  toChanged(newValue: NgbDateStruct) {
    this.refreshData();
  }

  toTimeChanged(newValue: NgbDateStruct) {
    this.refreshData();
  }

  formatUnitScanCreatedOn(createdOn: string) {
    return moment(createdOn).calendar();
  }

  getGoogleMapsLatLngForUnitScan(unitScan: IUnitScan) {
    return new google.maps.LatLng(unitScan.gpsLatitude, unitScan.gpsLongitude);
  }

  getTagScansIds(tagScans: ITagScan[]) {
    return tagScans.map(x => x.tagId).join(', ');
  }

  openPicModal(picModalContent: TemplateRef<any>, imageJpegBase64: string) {
    this.imageJpegBase64 = imageJpegBase64;
    this.modalService.open(picModalContent);
  }

  tagsFilterChanged(event: any) {
    this.refreshData();
  }

  exportToExcel() {
    const unitScansCopy = this.unitScans.map(x => Object.assign({}, x));
    // remove unused properties
    unitScansCopy.forEach(unitScan => {
      delete unitScan['id'];
      delete unitScan['unitInfo']['assignedUsers'];
      delete unitScan['imageJpegBase64'];
      delete unitScan['connection'];
      delete unitScan['gpsLatitude'];
      delete unitScan['gpsLongitude'];
      delete unitScan['ownerId'];
      delete unitScan['owner'];
      delete unitScan['tagScans'];
    });

    this.excelService.exportAsExcelFile(unitScansCopy.map(unitScan => flat(unitScan)), 'unit-scans-export');
  }

  getUnitScansWithGps() {
    return this.unitScans.filter(unitScan => unitScan.gpsLongitude && unitScan.gpsLatitude);
  }

  getOwnerUsername(unitScan: IUnitScan) {
    if (unitScan.owner) {
      return unitScan.owner.username;
    } else {
      return '';
    }
  }

  stateFilterChanged(event: any) {
    this.refreshData();
  }

  getIconForUnitScan(unitScan: IUnitScan) {
    return {url: 'https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|' + unitScan.unitInfo.hexRgbColor};
  }

  openInfo(marker: MapMarker, unitScan: IUnitScan) {
    this.infoContent = this.getUnitScanMapLabel(unitScan);
    this.infoWindow.open(marker);
  }

  getUnitScanMapLabel(unitScan: IUnitScan) {
    const name = unitScan.unitInfo.name;
    const tags = unitScan.tagScans.map(x => x.tagId).join(',');
    const time = unitScan.createdOn;

    return 'Name: ' + name + '\n' + 'Tags: ' + tags + '\n' + 'Time: ' + time;
  }
}
