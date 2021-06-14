import {Component, OnInit} from '@angular/core';
import {Observable} from 'rxjs';
import {AuthorizeService} from '../../../api-authorization/authorize.service';
import {map} from 'rxjs/operators';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit {
  public userName: Observable<string>;

  constructor(private authorizeService: AuthorizeService) {
  }

  ngOnInit() {
    this.userName = this.authorizeService.getUser().pipe(map(u => u && u.name));
  }
}
