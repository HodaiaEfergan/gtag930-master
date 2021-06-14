import {IRole} from './irole';

export interface IUser {
  id: string;
  username: string;
  roles: IRole[];
  isLocked: boolean;
}

