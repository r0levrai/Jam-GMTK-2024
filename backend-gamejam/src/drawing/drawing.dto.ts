import { IsNotEmpty, IsOptional } from "class-validator";
import { Points, Vector3 } from "./drawing.entity";

export class DrawingDto {
    @IsNotEmpty()
    linesPoints: Points[];

    @IsNotEmpty()
    linesWidth: number[];
    
    @IsNotEmpty()
    linesColorIndex: number[];

    @IsOptional()
    userName: string;

    @IsOptional()
    drawingName: string;

    @IsOptional()
    background: string;

    @IsOptional()
    score: number;
}