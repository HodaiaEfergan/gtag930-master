import {IUser} from './iuser';

export interface IUnitInfo {
  id: string;
  name: string;
  assignedUsers: IUser[];
  hexRgbColor: string;
}

