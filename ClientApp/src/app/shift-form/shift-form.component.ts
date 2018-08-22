import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-shift-form',
  templateUrl: './shift-form.component.html'
})
export class ShiftFormComponent {

  public shifts: ShiftToShow[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Shift[]>(baseUrl + 'api/shifts').subscribe(result => {

      result.forEach(element => {
        var existRow = this.shifts.find(s => s.date == new Date(element.date).toDateString());
        if (existRow) {
          if (element.slot == 1) {
            existRow.morningSlot = element.engineerName;
          } else
            existRow.afternoonSlot = element.engineerName;
        } else {
          var row = new ShiftToShow();
          row.id = element.id;
          row.date = new Date(element.date).toDateString();
          if (element.slot == 1) {
            row.morningSlot = element.engineerName;
          } else
            row.afternoonSlot = element.engineerName;

          this.shifts.push(row);
        }
      });
    }, error => console.error(error));
  }
}

interface Shift {
  id: number;
  date: string,
  slot: number;
  engineerName: string;
}

class ShiftToShow {
  id: number;
  date: string;
  morningSlot: string;
  afternoonSlot: string;
}