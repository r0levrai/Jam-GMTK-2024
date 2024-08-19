import { Controller, Get, Post, Put, Delete, Param, Body, Query } from "@nestjs/common";
import { DrawingService } from "./drawing.service";
import { Drawing } from "./drawing.entity";
import { DrawingDto } from "./drawing.dto";

@Controller('drawings')
export class DrawingController {
    constructor(private readonly drawingService: DrawingService) {}

    @Post()
    async createDrawing(@Body() drawingDto: DrawingDto): Promise<Drawing> {
        return await this.drawingService.createDrawing(drawingDto);
    }

    @Get('lasts')
    async getLastDrawings(@Query('n') n: number): Promise<Drawing[]> {
        return await this.drawingService.getLastDrawings(n);
    }

    @Get('firsts')
    async getFirstDrawings(@Query('n') n: number): Promise<Drawing[]> {
        return await this.drawingService.getFirstDrawings(n);
    }

    @Get('random')
    async getRandomDrawings(@Query('n') n: number): Promise<Drawing[]> {
        return await this.drawingService.getRandomDrawings(n);
    }

    @Get('user/:userName')
    async getDrawingsByUserName(@Param('userName') userName: string): Promise<Drawing[]> {
        return await this.drawingService.getDrawingsByUserName(userName);
    }

    @Put(':id')
    async updateDrawing(@Param('id') id: number, @Body() updateData: Partial<DrawingDto>): Promise<Drawing> {
        return await this.drawingService.updateDrawing(id, updateData);
    }

    @Delete(':id')
    async deleteDrawing(@Param('id') id: number): Promise<void> {
        await this.drawingService.deleteDrawing(id);
    }
}