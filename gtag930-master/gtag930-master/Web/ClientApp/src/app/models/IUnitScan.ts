import {IUnitInfo} from './IUnitInfo';
import {ITagScan} from './ITagScan';
import {IUser} from "./iuser";

export class IUnitScan {
  id: string;
  createdOn: string;
  unitInfo: IUnitInfo;
  description: string;
  imageJpegBase64: string;
  batteryVolt: number;
  connection: boolean;
  rssi: number;
  networkConnection: string;
  temperatureSensor1Celsius: number;
  temperatureSensor2Celsius: number;
  gpsLatitude: number;
  gpsLongitude: number;
  tagScans: ITagScan[];
  owner: IUser;
  state: string;
}
