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


    @Get('last10')
    async getLast10Drawings(): Promise<Drawing[]> {
        console.log("Getting last 10 drawings");
        return await this.drawingService.getLast10Drawings();
    }

    @Get('first10')
    async getFirst10Drawings(): Promise<Drawing[]> {
        return await this.drawingService.getFirst10Drawings();
    }

    @Get('random')
    async getRandomDrawings(@Query('limit') limit: number): Promise<Drawing[]> {
        return await this.drawingService.getRandomDrawings(limit);
    }
    
    @Get(':id')
    async getDrawingById(@Param('id') id: number): Promise<Drawing | undefined> {
        return await this.drawingService.getDrawingById(id);
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