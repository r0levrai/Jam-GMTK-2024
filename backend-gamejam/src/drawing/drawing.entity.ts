import { BaseEntity, Column, CreateDateColumn, Entity, PrimaryGeneratedColumn, ValueTransformer } from "typeorm";
import { DrawingResponseDto } from "./drawing.response.dto";



export class Vector3 {
    x: number;
    y: number;
    z: number;
}

export class Points {
    points: Vector3[];
}

export enum ReactionType {
    LIKE = "like",
    FUNNY = "funny",
    BAD = "bad",
}

const mapTransformer: ValueTransformer = {
    to: (value: Map<string, number>) => JSON.stringify(Array.from(value.entries())),
    from: (value: string) => new Map<string, number>(JSON.parse(value))
};

@Entity()
export class Drawing extends BaseEntity {
    @PrimaryGeneratedColumn()
    id: number;

    @CreateDateColumn()
    createdDate: Date;

    @Column("json")
    linesPoints: Points[];

    @Column("simple-array")
    linesWidth: number[];

    @Column("simple-array")
    linesColorIndex: number[];

    @Column()
    userName: string;

    @Column()
    drawingName: string;

    @Column()
    background: string;

    @Column()
    score: number;

    @Column("simple-array", {default: 0})
    like: string[];

    @Column("simple-array", {default: 0})
    funny: string[];

    @Column("simple-array", {default: 0})
    bad: string[];
}

export function convertDrawingToDto(drawing: Drawing): DrawingResponseDto {
    const drawingResponseDto = new DrawingResponseDto();
    
    drawingResponseDto.id = drawing.id;
    drawingResponseDto.createdDate = drawing.createdDate;
    drawingResponseDto.linesPoints = drawing.linesPoints;
    drawingResponseDto.linesWidth = drawing.linesWidth;
    drawingResponseDto.linesColorIndex = drawing.linesColorIndex;
    drawingResponseDto.userName = drawing.userName;
    drawingResponseDto.drawingName = drawing.drawingName;
    drawingResponseDto.background = drawing.background;
    drawingResponseDto.score = drawing.score;
    drawingResponseDto.like = drawing.like.length;
    drawingResponseDto.funny = drawing.funny.length;
    drawingResponseDto.bad = drawing.bad.length;
  
    return drawingResponseDto;
  }