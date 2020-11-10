import { Component, Input, OnInit } from '@angular/core';
import { Field, FieldType } from 'src/app/models/field';

@Component({
  selector: 'app-field-list-editor',
  templateUrl: './field-list-editor.component.html',
  styleUrls: ['./field-list-editor.component.scss']
})
export class FieldListEditorComponent implements OnInit {
  FieldType = FieldType;
  allFieldTypes: FieldType[] = [
    FieldType.String,
    FieldType.Integer,
    FieldType.Date,
    FieldType.Boolean
  ];

  @Input() fields: Field[];
  @Input() disabled: boolean;

  constructor() { }

  ngOnInit(): void {
  }

  moveUp(field: Field) {
    let ix = this.fields.indexOf(field);
    if (field.Index > 0 && ix > 0) {
      field.Index -= 1;
      let previous = this.fields[ix - 1];
      previous.Index = field.Index + 1;
      this.fields.splice(ix - 1, 2, field, previous);
    }
  }

  moveDown(field: Field) {
    let ix = this.fields.indexOf(field);
    if (ix + 1 < this.fields.length) {
      field.Index += 1;
      let next = this.fields[ix + 1];
      next.Index = field.Index - 1;
      this.fields.splice(ix, 2, next, field);
    }
  }

  getFieldTypeString(type: FieldType): string {
    return type.toString();
  }

  addField() {
    let nextIndex = this.fields.length > 0 ? this.fields[this.fields.length - 1].Index + 1 : 0;
    this.fields.push(Field.fromPayload({ Index: nextIndex }));
  }

  removeField(field: Field) {
    this.fields =
      this.fields.filter(f => f !== field)
        .map((f, ix) => {
          f.Index = ix;
          return f;
        });
  }
}
