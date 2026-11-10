import React, { useState, useEffect } from 'react';
import styles from './HumanResourceManagement.module.scss';
import { HeaderContainer, HeaderItem, Separator } from '../../common/components/Header';
import ContentContainer from '../../common/components/ContentContainer';
import { TabContainer, TabButton } from '../../common/components/Tab';
import { Table, TableHeader, TableCell } from '../../common/components/Table';
import CreateButton from '../../common/components/Button/CreateButton';
import { FaFileExport, FaChevronLeft, FaChevronRight, FaSearch } from 'react-icons/fa';
import { getPlanMaterials, getPlanPersonals, getSelectedPlanMaterials, getSelectedPlanPersonals } from '../../api/MaterialPersonal';
import moment from 'moment';
import * as XLSX from 'xlsx';

const HumanResourceManagement = () => {
    const [activeTab, setActiveTab] = useState('nguyen-vat-lieu');
    const [timeFilter, setTimeFilter] = useState('daily');
    const [tableData, setTableData] = useState({ headers: [], data: [] });
    const [loading, setLoading] = useState(false);

    // State dùng chung cho chế độ Monthly
    const [monthlyRawData, setMonthlyRawData] = useState(null);
    const [selectedWeekIndex, setSelectedWeekIndex] = useState(0);

    // --- NEW: State cho bộ lọc Tháng/Năm tùy chỉnh ---
    const [selectedMonth, setSelectedMonth] = useState(moment().month() + 1);
    const [selectedYear, setSelectedYear] = useState(moment().year());

    // Reset dữ liệu khi chuyển Tab
    useEffect(() => {
        setTableData({ headers: [], data: [] });
        setMonthlyRawData(null);
        setSelectedWeekIndex(0);
    }, [activeTab]);

    // --- 1. USE EFFECT CHÍNH: Load dữ liệu mặc định (Hiện tại) ---
    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            try {
                const timeRangeParam = timeFilter === 'daily' ? 0 : timeFilter === 'weekly' ? 1 : 2;
                let response;
                
                // Gọi API mặc định (theo logic thời gian hiện tại của Server)
                if (activeTab === 'nguyen-vat-lieu') {
                    response = await getPlanMaterials(timeRangeParam);
                } else {
                    response = await getPlanPersonals(timeRangeParam);
                }

                if (timeFilter === 'monthly') {
                    setMonthlyRawData(response?.monthlyPlan || null);
                    setSelectedWeekIndex(0);
                    
                    // Reset dropdown về tháng hiện tại để đồng bộ với dữ liệu mặc định vừa tải
                    setSelectedMonth(moment().month() + 1);
                    setSelectedYear(moment().year());
                } else {
                    setMonthlyRawData(null);
                    let processedData;
                    if (activeTab === 'nguyen-vat-lieu') {
                        processedData = processMaterialData(response, timeFilter);
                    } else {
                        processedData = processPersonnelData(response, timeFilter);
                    }
                    setTableData(processedData);
                }

            } catch (error) {
                console.error("Lỗi khi tải dữ liệu:", error);
                setTableData({ headers: [], data: [] });
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [activeTab, timeFilter]);

    // --- 2. HÀM XỬ LÝ: Khi người dùng chọn Tháng/Năm và bấm "Xem" ---
    const handleViewCustomMonth = async () => {
        setLoading(true);
        try {
            let response;
            // Gọi API Custom (getSelected...)
            if (activeTab === 'nguyen-vat-lieu') {
                response = await getSelectedPlanMaterials(selectedMonth, selectedYear);
            } else {
                response = await getSelectedPlanPersonals(selectedMonth, selectedYear);
            }

            // Cập nhật dữ liệu mới vào state monthlyRawData
            setMonthlyRawData(response?.monthlyPlan || response || null);
            setSelectedWeekIndex(0); // Reset về tuần 1

        } catch (error) {
            console.error("Lỗi khi tải dữ liệu tùy chọn:", error);
            setMonthlyRawData(null);
        } finally {
            setLoading(false);
        }
    };

    // --- 3. XỬ LÝ HIỂN THỊ: Chuyển đổi dữ liệu Monthly raw sang Table Data ---
    useEffect(() => {
        if (timeFilter === 'monthly' && monthlyRawData && monthlyRawData.items) {
            // Đảm bảo index không vượt quá giới hạn (ví dụ tháng cũ có 5 tuần, tháng mới có 4 tuần)
            let safeIndex = selectedWeekIndex;
            if (safeIndex >= monthlyRawData.items.length) {
                safeIndex = 0;
                setSelectedWeekIndex(0);
            }

            const currentWeekData = monthlyRawData.items[safeIndex];
            
            if (currentWeekData) {
                let processedData;
                if (activeTab === 'nguyen-vat-lieu') {
                    processedData = processMonthlyMaterialWeekData(currentWeekData);
                } else {
                    processedData = processMonthlyPersonnelWeekData(currentWeekData);
                }
                setTableData(processedData);
            }
        } else if (timeFilter === 'monthly' && !monthlyRawData) {
            setTableData({ headers: [], data: [] });
        }
    }, [selectedWeekIndex, monthlyRawData, timeFilter, activeTab]);

    // ========================================================================
    // LOGIC XUẤT EXCEL
    // ========================================================================
    const handleExportExcel = () => {
        if (!tableData || !tableData.data || tableData.data.length === 0) {
            alert("Không có dữ liệu để xuất!");
            return;
        }

        let titleInfo = "";
        let fileNameSuffix = "";
        const tabName = activeTab === 'nguyen-vat-lieu' ? 'NVL' : 'NhanSu';

        if (timeFilter === 'monthly' && monthlyRawData && monthlyRawData.items) {
            const currentWeekItem = monthlyRawData.items[selectedWeekIndex];
            const currentMonth = monthlyRawData.month;
            titleInfo = `Báo cáo ${tabName} - Tuần ${currentWeekItem?.week} / Tháng ${currentMonth}`;
            fileNameSuffix = `_Tuan${currentWeekItem?.week}_Thang${currentMonth}`;
        } else {
            const modeText = timeFilter === 'daily' ? 'Ngày' : 'Tuần';
            titleInfo = `Báo cáo ${tabName} - Theo ${modeText}`;
            fileNameSuffix = `_${timeFilter}`;
        }

        const dataForExport = tableData.data.map(row => {
            const newRow = {};
            tableData.headers.forEach(header => {
                // Đảm bảo dữ liệu xuất ra Excel là chuỗi (đã được format)
                newRow[header] = row[header];
            });
            return newRow;
        });

        const worksheet = XLSX.utils.json_to_sheet(dataForExport, { origin: "A2" });
        XLSX.utils.sheet_add_aoa(worksheet, [[titleInfo]], { origin: "A1" });

        if (tableData.headers.length > 1) {
            const mergeRange = { s: { r: 0, c: 0 }, e: { r: 0, c: tableData.headers.length - 1 } };
            if (!worksheet['!merges']) worksheet['!merges'] = [];
            worksheet['!merges'].push(mergeRange);
        }

        const wscols = [{ wch: 25 }, { wch: 15 }];
        for (let i = 0; i < 35; i++) wscols.push({ wch: 12 });
        worksheet['!cols'] = wscols;

        const workbook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, "Sheet1");

        const dateStr = moment().format('DDMMYYYY');
        const fileName = `QuanLyNguonLuc_${tabName}${fileNameSuffix}_${dateStr}.xlsx`;

        XLSX.writeFile(workbook, fileName);
    };

    // ========================================================================
    // HELPERS & LOGIC XỬ LÝ DATA (NVL + NHÂN SỰ)
    // ========================================================================
    
    // Helper tạo danh sách Năm cho Dropdown
    const generateYearOptions = () => {
        // Lấy năm hiện tại bằng moment
        const currentYear = moment().year();
        const years = [];

        // Định nghĩa phạm vi năm
        const startYear = currentYear - 10; // Bắt đầu từ 10 năm trước
        const endYear = currentYear + 2;   // Kết thúc 2 năm sau

        // Lặp qua phạm vi và thêm vào mảng
        for (let i = startYear; i <= endYear; i++) {
            years.push(i);
        }

        return years;
    };

    // --- Logic NVL ---
    const processMaterialData = (data, type) => {
        let timeHeaders = [];
        let rows = {};

        if (type === 'daily' && data?.dailyPlan?.items) {
            const uniqueDates = [...new Set(data.dailyPlan.items.map(item => moment(item.planDate).format('DD/MM')))].sort();
            timeHeaders = uniqueDates;
            data.dailyPlan.items.forEach(item => {
                const dateKey = moment(item.planDate).format('DD/MM');
                updateMaterialRow(rows, item, dateKey, timeHeaders);
            });
        }
        else if (type === 'weekly' && data?.weeklyPlan?.items) {
            timeHeaders = data.weeklyPlan.items.map(day => moment(day.date).format('DD/MM'));
            data.weeklyPlan.items.forEach(dayData => {
                const dateKey = moment(dayData.date).format('DD/MM');
                if (dayData.items) {
                    dayData.items.forEach(item => updateMaterialRow(rows, item, dateKey, timeHeaders));
                }
            });
        }
        return formatMaterialOutput(rows, timeHeaders);
    };

    const processMonthlyMaterialWeekData = (weekData) => {
        let timeHeaders = [];
        let rows = {};
        if (weekData && weekData.items) {
            timeHeaders = weekData.items.map(day => moment(day.date).format('DD/MM'));
            weekData.items.forEach(dayData => {
                const dateKey = moment(dayData.date).format('DD/MM');
                if (dayData.items) {
                    dayData.items.forEach(item => updateMaterialRow(rows, item, dateKey, timeHeaders));
                }
            });
        }
        return formatMaterialOutput(rows, timeHeaders);
    };

    const updateMaterialRow = (rows, item, timeKey, headers) => {
        const materialKey = item.materialName + ' [' + item.materialId + ']';
        if (!rows[materialKey]) {
            rows[materialKey] = {
                'NVL': materialKey + ' (' + (item.unit || '--') + ')',
                'rawTotal': 0,
                'unit': item.unit || '',
            };
            headers.forEach(h => rows[materialKey][h] = 0);
        }
        const row = rows[materialKey];
        row.rawTotal += item.quantity;
        row[timeKey] = (row[timeKey] || 0) + item.quantity;
    };

    const formatMaterialOutput = (rows, timeHeaders) => {
        const formattedData = Object.values(rows).map(row => {
            const newRow = { 'NVL': row['NVL'] };
            const unit = row.unit ? ` ${row.unit}` : '';
            timeHeaders.forEach(header => {
                const val = row[header];
                newRow[header] = val > 0 ? `${val.toFixed(2)}` : '-';
            });
            newRow['Tổng'] = row.rawTotal > 0 ? `${row.rawTotal.toFixed(2)}` : '-';
            return newRow;
        });
        return { headers: ['NVL', ...timeHeaders, 'Tổng'], data: formattedData };
    };

    // --- Logic Nhân sự ---
    const processPersonnelData = (data, type) => {
        let timeHeaders = [];
        let rows = {};

        if (type === 'daily' && data?.dailyPlan?.items) {
            const uniqueDates = [...new Set(data.dailyPlan.items.map(item => moment(item.planDate).format('DD/MM')))].sort();
            timeHeaders = uniqueDates;
            data.dailyPlan.items.forEach(item => {
                const dateKey = moment(item.planDate).format('DD/MM');
                updatePersonnelRow(rows, item, dateKey, timeHeaders);
            });
        }
        else if (type === 'weekly' && data?.weeklyPlan?.items) {
            timeHeaders = data.weeklyPlan.items.map(day => moment(day.date).format('DD/MM'));
            data.weeklyPlan.items.forEach(dayData => {
                const dateKey = moment(dayData.date).format('DD/MM');
                if (dayData.items) {
                    dayData.items.forEach(item => {
                        updatePersonnelRow(rows, item, dateKey, timeHeaders);
                    });
                }
            });
        }
        return formatPersonnelOutput(rows, timeHeaders);
    };

    const processMonthlyPersonnelWeekData = (weekData) => {
        let timeHeaders = [];
        let rows = {};
        if (weekData && weekData.items) {
            timeHeaders = weekData.items.map(day => moment(day.date).format('DD/MM'));
            weekData.items.forEach(dayData => {
                const dateKey = moment(dayData.date).format('DD/MM');
                if (dayData.items && dayData.items.length > 0) {
                    dayData.items.forEach(item => {
                        updatePersonnelRow(rows, item, dateKey, timeHeaders);
                    });
                }
            });
        }
        return formatPersonnelOutput(rows, timeHeaders);
    };

    const formatShiftName = (shift) => {
        if (shift === 'Ca12') return 'Ca 1, Ca 2';
        if (shift === 'Ca13') return 'Ca 1, Ca 3';
        if (shift === 'Ca23') return 'Ca 2, Ca 3';
        return shift;
    };

    const updatePersonnelRow = (rows, item, timeKey, headers) => {
        const personId = item.personnelId;
        if (!rows[personId]) {
            rows[personId] = {
                'personnelName': item.personnelName,
                'personnelId': item.personnelId,
                'rawShifts': {}
            };
            headers.forEach(h => rows[personId]['rawShifts'][h] = []);
        }
        const formattedShift = formatShiftName(item.shift);
        if (rows[personId]['rawShifts'][timeKey] !== undefined) {
            if (!rows[personId]['rawShifts'][timeKey].includes(formattedShift)) {
                rows[personId]['rawShifts'][timeKey].push(formattedShift);
            }
        }
    };

    const formatPersonnelOutput = (rows, timeHeaders) => {
        const formattedData = Object.values(rows).map(row => {
            const newRow = {
                'Tên nhân viên': row.personnelName,
                'Mã nhân viên': row.personnelId
            };
            timeHeaders.forEach(header => {
                const shifts = row.rawShifts[header];
                newRow[header] = (shifts && shifts.length > 0) ? shifts.join(', ') : '-';
            });
            // Thêm cột tổng chỉ khi chế độ Nhân sự (để khớp với logic hiện tại, mặc dù tổng không rõ ràng)
            // Nếu bạn muốn hiển thị tổng số ca làm, cần thay đổi logic xử lý rawTotal
            return newRow;
        });
        
        // Cần điều chỉnh headers cho Nhân sự để khớp với data
        const personnelHeaders = activeTab === 'nguyen-vat-lieu' ? [] : ['Tên nhân viên', 'Mã nhân viên', ...timeHeaders];

        // Nếu là NVL, thêm cột Tổng. Nếu là Nhân sự, không cần cột Tổng (vì là ca/kíp)
        if (activeTab === 'nguyen-vat-lieu') {
            return { headers: ['NVL', ...timeHeaders, 'Tổng'], data: formattedData };
        } else {
            return { headers: personnelHeaders, data: formattedData };
        }
    };

    // ========================================================================
    // UI HANDLERS
    // ========================================================================
    const handlePrevWeek = () => {
        if (selectedWeekIndex > 0) setSelectedWeekIndex(prev => prev - 1);
    };

    const handleNextWeek = () => {
        if (monthlyRawData && monthlyRawData.items && selectedWeekIndex < monthlyRawData.items.length - 1) {
            setSelectedWeekIndex(prev => prev + 1);
        }
    };

    return (
        <div className={styles.pageContainer}>
            <h2 className="title">{activeTab === 'nguyen-vat-lieu' ? ' Kế hoạch tài nguyên | Nguyên vật liệu' : ' Kế hoạch tài nguyên | Nhân sự'}</h2>
            
            <TabContainer className={styles.mainTabContainer}>
                <TabButton active={activeTab === 'nguyen-vat-lieu'} onClick={() => setActiveTab('nguyen-vat-lieu')}>Nguyên vật liệu</TabButton>
                <TabButton active={activeTab === 'nhan-su'} onClick={() => setActiveTab('nhan-su')}>Nhân sự</TabButton>
            </TabContainer>

            <ContentContainer className={styles.mainContent}>
                
                <div className={styles.tableHeaderSection}>
                    {/* Tiêu đề Bảng */}
                    <h3 className={styles.sectionTitle}>
                        Bảng {activeTab === 'nguyen-vat-lieu' ? 'NVL' : 'Nhân sự'} - Theo {timeFilter === 'daily' ? 'Ngày' : timeFilter === 'weekly' ? 'Tuần' : 'Tháng'}
                    </h3>

                    <div className={styles.controls}>
                        
                        {/* --- Lựa chọn Tháng/Năm tùy chỉnh --- */}
                        {timeFilter === 'monthly' && (
                            <div className={styles.customFilterGroup}>
                                <select 
                                    value={selectedMonth} 
                                    onChange={(e) => setSelectedMonth(parseInt(e.target.value))}
                                    className={styles.dropdown}
                                >
                                    {Array.from({ length: 12 }, (_, i) => i + 1).map(m => (
                                        <option key={m} value={m}>Tháng {m}</option>
                                    ))}
                                </select>

                                <select 
                                    value={selectedYear} 
                                    onChange={(e) => setSelectedYear(parseInt(e.target.value))}
                                    className={styles.dropdown}
                                >
                                    {generateYearOptions().map(y => (
                                        <option key={y} value={y}>Năm {y}</option>
                                    ))}
                                </select>

                                <button onClick={handleViewCustomMonth} className={styles.viewButton}>
                                    <FaSearch size={12} /> Xem
                                </button>
                            </div>
                        )}

                        {/* --- Điều hướng Tuần --- */}
                        {timeFilter === 'monthly' && monthlyRawData?.items && (
                            <div className={styles.weekNavGroup}>
                                <button 
                                    onClick={handlePrevWeek} 
                                    disabled={selectedWeekIndex === 0} 
                                    className={styles.navBtn}
                                >
                                    <FaChevronLeft />
                                </button>
                                <select 
                                    value={selectedWeekIndex} 
                                    onChange={(e) => setSelectedWeekIndex(parseInt(e.target.value))} 
                                    className={styles.dropdown}
                                >
                                    {monthlyRawData.items.map((weekItem, index) => (
                                        <option key={index} value={index}>Tuần {weekItem.week} / T{monthlyRawData.month}</option>
                                    ))}
                                </select>
                                <button 
                                    onClick={handleNextWeek} 
                                    disabled={selectedWeekIndex === monthlyRawData.items.length - 1} 
                                    className={styles.navBtn}
                                >
                                    <FaChevronRight />
                                </button>
                            </div>
                        )}
                        
                        {/* --- Bộ lọc Ngày/Tuần/Tháng (Segmented Control) --- */}
                        <div className={styles.timeFilterButtons}>
                            <button 
                                className={timeFilter === 'daily' ? styles.active : ''} 
                                onClick={() => setTimeFilter('daily')}
                            >Ngày</button>
                            <button 
                                className={timeFilter === 'weekly' ? styles.active : ''} 
                                onClick={() => setTimeFilter('weekly')}
                            >Tuần</button>
                            <button 
                                className={timeFilter === 'monthly' ? styles.active : ''} 
                                onClick={() => setTimeFilter('monthly')}
                            >Tháng</button>
                        </div>

                        {/* --- Nút Xuất Excel --- */}
                        <CreateButton className={styles.exportButton} onClick={handleExportExcel}>
                            <FaFileExport /> Xuất Excel
                        </CreateButton>
                    </div>
                </div>

                <div className={styles.tableContainer}>
                    {loading ? (
                    <div className={styles.loadingWrapper}>
                        <div className={styles.loader}></div>
                        <p className={styles.loadingText}>Đang xử lý dữ liệu...</p>
                    </div>
                    ) : (
                        <Table>
                            <thead>
                                <tr>
                                    {tableData.headers.map((header, idx) => (
                                        <TableHeader 
                                            key={idx} 
                                            // Đánh dấu cột đầu tiên để làm sticky
                                            className={idx === 0 ? styles.stickyHeaderCol : ''} 
                                            style={{ whiteSpace: 'nowrap' }}
                                        >
                                            {header}
                                        </TableHeader>
                                    ))}
                                </tr>
                            </thead>
                            <tbody>
                                {tableData.data.length > 0 ? (
                                    tableData.data.map((row, rowIndex) => (
                                        <tr key={rowIndex}>
                                            {tableData.headers.map((header, colIndex) => (
                                                <TableCell 
                                                    key={`${rowIndex}-${header}`}
                                                    // Đánh dấu cột đầu tiên để làm sticky
                                                    className={colIndex === 0 ? styles.stickyBodyCol : ''} 
                                                >
                                                    {row[header]}
                                                </TableCell>
                                            ))}
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan={tableData.headers.length || 1} className={styles.noDataCell}>
                                            Không có dữ liệu
                                        </td>
                                    </tr>
                                )}
                            </tbody>
                        </Table>
                    )}
                </div>
            </ContentContainer>
        </div>
    );
};

export default HumanResourceManagement;