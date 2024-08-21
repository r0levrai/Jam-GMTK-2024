import { Type } from "class-transformer";
import { IsEnum, IsInt, IsOptional, Max, Min } from "class-validator";


export enum Order {
    ASC = "ASC",
    DESC = "DESC",
  }

export class PageOptionsDto {
  @IsEnum(Order)
  @IsOptional()
  readonly order?: Order = Order.DESC;
  

  @Type(() => Number)
  @IsInt()
  @Min(1)
  @IsOptional()
  readonly page?: number = 1;

  @Type(() => Number)
  @IsInt()
  @Min(1)
  @Max(50)
  @IsOptional()
  readonly take?: number = 7;

  @Type(() => Boolean)
  @IsOptional()
  readonly all?: boolean = false;

  get skip(): number {
    return (this.page - 1) * this.take;
  }
}