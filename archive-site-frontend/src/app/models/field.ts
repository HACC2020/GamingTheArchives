import { Model } from 'src/app/models/model';
import { $enum } from 'ts-enum-util';

const langEn = 'en';
const langHaw = 'haw';

export class Field extends Model {
  get englishHelp(): string {
    return this.getHelpText()[langEn]
  }

  set englishHelp(helpText: string) {
    this.HelpTextData = JSON.stringify({
      ...this.getHelpText(),
      'en': helpText
    });
  }

  get hawaiianHelp(): string {
    return this.getHelpText()[langHaw]
  }

  set hawaiianHelp(helpText: string) {
    this.HelpTextData = JSON.stringify({
      ...this.getHelpText(),
      'haw': helpText
    });
  }

  constructor(
    Id: number = 0,
    public ProjectId: number = 0,
    public Name: string = undefined,
    public HelpTextData: string = undefined,
    public Index: number = 0,
    public Type: FieldType = FieldType.String,
    public Required: boolean = false,
    public ParsingFormat?: string,
    public TrueValue?: string,
    public FalseValue?: string,
    public ValidationData?: string) {
    super(Id);
  }

  getHelpText(): { [key: string]: string } {
    if (this.HelpTextData) {
      return JSON.parse(this.HelpTextData);
    } else {
      return {};
    }
  }

  getValidationData(): IValidation[] {
    if (this.ValidationData) {
      let validations = <IValidation[]> JSON.parse(this.ValidationData);

      return validations.map(v => updateConfiguration(v));
    }
  }

  static fromPayload(payload: any): Field {
    return new Field(
      payload.Id,
      payload.ProjectId,
      payload.Name,
      payload.HelpTextData,
      payload.Index,
      payload.Type ? toFieldType(payload.Type) : FieldType.String,
      payload.Required,
      payload.ParsingFormat,
      payload.TrueValue,
      payload.FalseValue,
      payload.ValidationData
    )
  }
}

function updateConfiguration(v: IValidation): IValidation {
  if (v.Configuration) {
    if (typeof v.Type === 'string') {
      v.Type = toValidationType(v.Type);
    }

    switch (v.Type) {
      case ValidationType.Length:
      case ValidationType.IntegerRange:
        if (typeof v.Configuration.Min === 'string') {
          v.Configuration.Min = Number(v.Configuration.Min);
        }
        if (typeof v.Configuration.Max === 'string') {
          v.Configuration.Max = Number(v.Configuration.Max);
        }
        break;
      case ValidationType.DateRange:
        if (typeof v.Configuration.Min === 'string') {
          v.Configuration.Min = Date.parse(v.Configuration.Min);
        }
        if (typeof v.Configuration.Max === 'string') {
          v.Configuration.Min = Date.parse(v.Configuration.Max);
        }
        break;
    }
  }

  return v;
}

export enum FieldType {
  Boolean = 'Boolean',
  Integer = 'Integer',
  String = 'String',
  Date = 'Date'
}

export function toFieldType(value: string): FieldType {
  return FieldType[$enum(FieldType).asKeyOrDefault(value)];
}

export interface IValidation {
  Type: ValidationType;
  Configuration: IValidationConfiguration;
  Condition: any;
}

export enum ValidationType {
  Length,
  Regex,
  IntegerRange,
  DateRange,
  RestrictedValue
}

export function toValidationType(value: string): ValidationType {
  return ValidationType[$enum(ValidationType).asKeyOrDefault(value)];
}

export interface IValidationConfiguration {
  Min: string | number | Date;
  Max: string | number | Date;
  AllowedValues: string[];
  Pattern: string[];
}
